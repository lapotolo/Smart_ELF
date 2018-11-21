import com.google.gson.Gson;
import elf_crawler.CrawlingManager;
import elf_crawler.URLSet;
import elf_crawler.crawler.DataEntry;
import elf_crawler.relationship.RdfRelation;
import elf_crawler.relationship.RelationshipSet;
import elf_crawler.util.Logger;
import elf_kb_protocol.Fact;
import elf_kb_protocol.JReq;
import elf_kb_protocol.KBConnection;
import elf_kb_protocol.KBTTL;

import java.io.*;
import java.util.List;
import java.util.Map;

public class Main {

    private static Gson gson = new Gson();

    public static void main(String[] args) throws Exception {
        int processors = Runtime.getRuntime().availableProcessors();
        System.err.println(String.format("Using %d threads", processors));

        URLSet urlSet = new URLSet("url-set.json");
        RelationshipSet rs = new RelationshipSet("relationship-set.json");
        CrawlingManager cs = new CrawlingManager(urlSet, rs);

        List<DataEntry> dataEntries = cs.executeAllCrawlers();
        cs.shutdown();

        System.err.println("All Crawlers have finished!");
        if (cs.hasNewLinks()) {
            System.err.println(String.format("Discovered %d new links!", cs.getNewLinkCount()));
        }

        KBConnection con = new KBConnection("ws://localhost", 5666);

        JReq jreq = new JReq();
        jreq.addTag("t1", "d1", "doc1");
        con.registerTags(jreq);

        for (DataEntry d : dataEntries) {
            if (d == null) continue;
            con.addFact(new Fact("t1", KBTTL.DAY, 100, true, d));
        }

        con.closeConnection();
    }

    public static String rdfDataToJson(Map<String, List<RdfRelation>> rdfData) {
        return gson.toJson(rdfData);
    }

    public static void saveURLSet(String filename, List<String> urls) throws IOException {
        FileWriter writer = new FileWriter(filename);
        writer.write(gson.toJson(urls));
        writer.close();
    }

}
